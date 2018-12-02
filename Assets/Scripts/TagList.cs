using System;
using System.Collections.Generic;

//Only used for deprecated message
using UnityEngine;

// A tagged item, used to hold a value and associated name and tags.
// To be used only with TagList
public struct Tagged<T>
{
    //Unique name given to this item
    public string name;

    //The tags associated with this item
    public string[] tags;

    // The actual value being stored
    public T value;

    public Tagged(string _name, T _value, string[] _tags)
    {
        name = _name;
        tags = _tags;
        value = _value;
    }
}

/**
    @class TagList
    
    Stores values with tags for easier finding of several items with the same tags.
 **/
public class TagList<T> where T : class
{
    //List of tagged items
    List<Tagged<T>> list = new List<Tagged<T>>();

    public Tagged<T> this[int i]
    {
        get { return list[i];  }
    }

    /**
        @function Add - Add a new item into the list
        @param tag - The tag to look for in the TagList

        @return - The value found, or null;
    **/
    public void Add(string name, T value, string[] tags)
    {
        list.Add(new Tagged<T>(name, value, tags));
    }


    /**
        @function FindFirst - returns a list of the first items that has @tag
        @param tag - The tag to look for in the TagList

        @return - The value found, or null;
    **/
    public T FindFirst(string tag)
    {
        foreach (var item in list)
        {
            if(Array.Find(item.tags, i => i.Equals(tag)) != null)
            {
                return item.value;
            }
        }
        return null;
    }

    /**
        @function FindAll - returns a list of all the items that have @tag
        @param tag - The tag to look for in the TagList

        @return - List of the values found, if none found the list will be empty.
    **/
    public List<T> FindAll(string tag)
    {
        List<T> found = new List<T>();

        foreach (var item in list)
        {
            if (Array.Find(item.tags, i => i.Equals(tag)) != null)
            {
                found.Add(item.value);
            }
        }

        return found;
    }

    /**
        @function FindName - Find an item with the given name.
        @param name - The name given to the item

        @returns - The item associated with that name, or null if none found.
    **/
    public T FindName(string name)
    {
        foreach (var item in list)
        {
            if (item.name == name)
            {
                return item.value;
            }
        }
        return null;
    }


    /**
        @function Execute - passes TagList to callback and index of items that have @tag
        @param tag - a string with the tag to search for in the TagList
        @param callback - of type Callback, is called by passing the Tag List and the index of the found items.
        
        For simpler syntax, look for the next Execute function
    **/
    public delegate void Callback(List<Tagged<T>> list, int index);

    public void Execute(string tag, Callback callback)
    {
        for(int i = list.Count - 1; i >= 0; i--)
        { 
            if (Array.Find(list[i].tags, o => o.Equals(tag)) != null)
            {
                callback(list, i);
            }
        }
    }


    /**
        @function Execute - passes the value to @callback found by the given @tag.
        @param tag - a string with the tag to search for in the TagList
        @param callback - of type RemoveCallback, is called by passing the items found with @tag,
          return true from the callback to remove the item from the list.
    **/
    public delegate bool RemoveCallback(T value);

    public void Execute(string tag, RemoveCallback callback)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (Array.Find(list[i].tags, o => o.Equals(tag)) != null)
            {
                if (callback(list[i].value))
                    list.RemoveAt(i);
            }
        }
    }
}