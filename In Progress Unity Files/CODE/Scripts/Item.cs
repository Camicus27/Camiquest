using System;

public class Item : IEquatable<Item>
{
    public string itemName = "Item";
    public string itemInfo = "This is an item";
    public string type = "DEFAULT";
    public int effectivenessPoints = 10;

    public Item()
    {
        itemName = "-";
        itemInfo = "";
        type = "NONE";
        effectivenessPoints = 0;
    }

    public Item(string name)
    {
        switch (name)
        {
            case "Health":
                itemName = "Health Potion";
                itemInfo = "Your run of the mill, absolutely standard, has been done a million times before, health potion.";
                type = "HEALTH";
                effectivenessPoints = 75;
                break;
            case "Mana":
                itemName = "Mana Potion";
                itemInfo = "Nobody really knows how this works, but drinking it makes you feel funny " +
                    "and suddenly your magic works again. Neat!";
                type = "MANA";
                effectivenessPoints = 75;
                break;
            case "Strength":
                itemName = "Strength Potion";
                itemInfo = "You get stronger for this fight. You probably could've guessed that from the name of the potion.";
                type = "STRENGTH";
                effectivenessPoints = 5;
                break;
            case "Toughness":
                itemName = "Toughness Potion";
                itemInfo = "Like strength for your enemies, but like opposite.";
                type = "TOUGHNESS";
                effectivenessPoints = 5;
                break;
        }
    }

    public Item(string name, string info, string type, int effectivenessAmt)
    {
        itemName = name;
        itemInfo = info;
        this.type = type;
        effectivenessPoints = effectivenessAmt;
    }

    public static bool operator ==(Item left, object otherObj)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(otherObj, null);

        return left.Equals(otherObj);
    }

    public static bool operator !=(Item left, object otherObj)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(otherObj, null);

        return left.Equals(otherObj);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Item))
            return false;
        return (obj as Item).itemName == itemName;
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }

    public bool Equals(Item other)
    {
        return other.itemName == itemName;
    }
}
