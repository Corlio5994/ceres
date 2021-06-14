using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : Container {
    private static Dictionary<int, Bank> banks = new Dictionary<int, Bank> ();

    public int id = -1;

    void Start () {
        if (banks.ContainsKey (id)) banks.Remove (id);

        banks.Add (id, this);

        Deposit (ItemDatabase.GetItem (0, 10));
        Deposit (ItemDatabase.GetItem (1, 10));
    }

    public static Bank Get (int id) {
        if (banks.ContainsKey (id))
            return banks[id];
        return null;
    }
}