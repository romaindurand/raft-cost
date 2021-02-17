using UnityEngine;
using System.Collections.Generic;

public class Raft_Counter : Mod
{
    IDictionary<string, int> itemsRequired;
    IDictionary<string, int> itemsIgnored;
    public void Start()
    {
        Debug.Log("Mod Raft_Counter has been loaded!");
    }

    public void OnModUnload()
    {
        Debug.Log("Mod Raft_Counter has been unloaded!");
    }

    [ConsoleCommand(name: "raftCost", docs: "List all the ressources needed to build your current raft.")]
    public static void raftCommand(string[] args)
    {
        IDictionary<string, int> itemsRequired = new Dictionary<string, int>();
        IDictionary<string, int> itemsIgnored = new Dictionary<string, int>();
        
        var blocks = UnityEngine.Object.FindObjectsOfType<Block>();
        for (int i = 0; i < blocks.Length; i++)
        {
            registerBlock(blocks[i], itemsRequired, itemsIgnored);
        }
        if (itemsIgnored.ContainsKey("RespawnPointBed"))
        {
            itemsIgnored.Remove("RespawnPointBed");
        }

        printItemsCounts("Required items : ", itemsRequired);

        if (itemsIgnored.Count == 0) return;
        printItemsCounts("Items ignored in count : ", itemsIgnored);
    }

    private static void printItemsCounts(string text, IDictionary<string, int> itemsCounts)
    {
        Debug.Log(text);
        foreach (KeyValuePair<string, int> item in itemsCounts)
        {
            Debug.Log("- " + item.Key + " : x" + item.Value);
        }
    }

    static void registerBlock(Block block, IDictionary<string, int> itemsRequired, IDictionary<string, int> itemsIgnored)
    {
        Item_Base baseItem = block.buildableItem;
        string blockName = "";
        if (baseItem == null)
        {
            blockName = block.name.Replace("(Clone)", "");
            baseItem = ItemManager.GetItemByName(blockName);
        }

        if (baseItem == null)
        {
            if (itemsIgnored.ContainsKey(blockName))
            {
                itemsIgnored[blockName]++;
            } else
            {
                itemsIgnored.Add(blockName, 1);
            }
            return;
        }

        if (block.Reinforced)
        {
            string metalIngot = "MetalIngot";
            if (!itemsRequired.ContainsKey(metalIngot))
            {
                itemsRequired.Add(metalIngot, 1);
            } else
            {
                itemsRequired[metalIngot] += 1;
            }

            string nail = "Nail";
            if (!itemsRequired.ContainsKey(nail))
            {
                itemsRequired.Add(nail, 2);
            }
            else
            {
                itemsRequired[nail] += 2;
            }
        }
        ItemInstance_Recipe recipe = baseItem.settings_recipe;
        CostMultiple[] costs = recipe.NewCost;
        if (costs == null) return;
        if (costs.Length == 0) return;

        for (int i = 0; i < costs.Length; i++)
        {
            Item_Base[] items = costs[i].items;
            int amount = costs[i].amount;
            if (items.Length == 0) continue;
            for (int j = 0; j < items.Length; j++)
            {
                if (itemsRequired.ContainsKey(items[j].name))
                {
                    itemsRequired[items[j].name]+= amount;
                } else
                {
                    itemsRequired.Add(items[j].name, amount);
                }
            }

        }
    }
}
