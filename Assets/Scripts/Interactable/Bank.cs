using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bank : Container {
    private static Dictionary<int, Bank> banks = new Dictionary<int, Bank> ();
    public static int[] GetBankIDs {get { return banks.Keys.ToArray(); } }

    public int id = -1;

    protected override void Start () {
        base.Start();
        
        if (banks.ContainsKey (id)) banks.Remove (id);

        banks.Add (id, this);
    }

    public static Bank Get (int id) {
        if (banks.ContainsKey (id))
            return banks[id];
        return null;
    }
}