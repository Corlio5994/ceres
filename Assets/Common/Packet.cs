using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Packet : IDisposable {

    private bool disposed = false;
    private List<byte> buffer = new List<byte> ();
    private byte[] readableBuffer;
    private int readPosition = 0;

    public void Dispose () {
        Dispose (true);
        GC.SuppressFinalize (this);
    }

    protected virtual void Dispose (bool disposing) {
        if (!disposed) {
            if (disposing) {
                buffer = null;
                readableBuffer = null;
                readPosition = 0;
            }

            disposed = true;
        }
    }

    public Packet () { }

    public Packet (ClientPackets id) {
        Console.Log (Enum.GetName (typeof (ClientPackets), id), "purple");
        Write ((int) id);
    }

    public Packet (ServerPackets id) {
        Console.Log (Enum.GetName (typeof (ServerPackets), id), "purple");
        Write ((int) id);
    }

    public Packet (byte[] data) {
        SetBytes (data);
    }

    public void SetBytes (byte[] data) {
        Write (data);
        readableBuffer = buffer.ToArray ();
    }

    public void WriteLength () {
        InsertInt (Length ());
    }

    public void InsertInt (int value) {
        byte[] bytes = BitConverter.GetBytes (value);
        buffer.InsertRange (0, bytes);
    }

    public byte[] ToArray () {
        readableBuffer = buffer.ToArray ();
        return readableBuffer;
    }

    public int Length () {
        return buffer.Count;
    }

    public int UnreadLength () {
        return Length () - readPosition;
    }

    public void Reset (bool shouldReset = true) {
        if (shouldReset) {
            buffer.Clear ();
            readableBuffer = null;
            readPosition = 0;
        } else {
            readPosition -= 4;
        }
    }

    #region Write
    public void Write (byte value) {
        buffer.Add (value);
    }

    public void Write (byte[] value) {
        buffer.AddRange (value);
    }

    public void Write (short value) {
        buffer.AddRange (BitConverter.GetBytes (value));
    }

    public void Write (int value) {
        buffer.AddRange (BitConverter.GetBytes (value));
    }

    public void Write (long value) {
        buffer.AddRange (BitConverter.GetBytes (value));
    }

    public void Write (float value) {
        buffer.AddRange (BitConverter.GetBytes (value));
    }

    public void Write (bool value) {
        buffer.AddRange (BitConverter.GetBytes (value));
    }

    public void Write (string value) {
        Write (value.Length);
        buffer.AddRange (Encoding.ASCII.GetBytes (value));
    }

    public void Write (Vector3 value) {
        Write (value.x);
        Write (value.y);
        Write (value.z);
    }

    public void Write (Quaternion value) {
        Write (value.x);
        Write (value.y);
        Write (value.z);
        Write (value.w);
    }

    public void Write (Item item) {
        Write (item.id);
        Write (item.count);
    }

    public void Write (Inventory inventory) {
        Write (inventory.itemCount);
        foreach (Item item in inventory.GetSortedItems ()) {
            Write (item);
        }
    }
    #endregion

    #region Read
    public byte ReadByte (bool moveReadPosition = true) {
        if (buffer.Count > readPosition) {
            byte value = readableBuffer[readPosition];
            if (moveReadPosition) {
                readPosition += 1;
            }
            return value;
        } else {
            throw new Exception ("Could not read value of type 'byte'!");
        }
    }

    public byte[] ReadBytes (int length, bool moveReadPosition = true) {
        if (buffer.Count > readPosition) {
            byte[] value = buffer.GetRange (readPosition, length).ToArray ();
            if (moveReadPosition) {
                readPosition += length;
            }
            return value;
        } else {
            throw new Exception ("Could not read value of type 'byte[]'!");
        }
    }

    public short ReadShort (bool moveReadPosition = true) {
        if (buffer.Count > readPosition) {
            short value = BitConverter.ToInt16 (readableBuffer, readPosition);
            if (moveReadPosition) {
                readPosition += 2;
            }
            return value;
        } else {
            throw new Exception ("Could not read value of type 'short'!");
        }
    }

    public int ReadInt (bool moveReadPosition = true) {
        if (buffer.Count > readPosition) {
            int value = BitConverter.ToInt32 (readableBuffer, readPosition);
            if (moveReadPosition) {
                readPosition += 4;
            }
            return value;
        } else {
            throw new Exception ("Could not read value of type 'int'!");
        }
    }

    public long ReadLong (bool moveReadPosition = true) {
        if (buffer.Count > readPosition) {
            long value = BitConverter.ToInt64 (readableBuffer, readPosition);
            if (moveReadPosition) {
                readPosition += 8;
            }
            return value;
        } else {
            throw new Exception ("Could not read value of type 'long'!");
        }
    }

    public float ReadFloat (bool moveReadPosition = true) {
        if (buffer.Count > readPosition) {
            float value = BitConverter.ToSingle (readableBuffer, readPosition);
            if (moveReadPosition) {
                readPosition += 4;
            }
            return value;
        } else {
            throw new Exception ("Could not read value of type 'float'!");
        }
    }

    public bool ReadBool (bool moveReadPosition = true) {
        if (buffer.Count > readPosition) {
            bool value = BitConverter.ToBoolean (readableBuffer, readPosition);
            if (moveReadPosition) {
                readPosition += 1;
            }
            return value;
        } else {
            throw new Exception ("Could not read value of type 'bool'!");
        }
    }

    public string ReadString (bool moveReadPosition = true) {
        try {
            int length = ReadInt ();
            string value = Encoding.ASCII.GetString (readableBuffer, readPosition, length);
            if (moveReadPosition && value.Length > 0) {
                readPosition += length;
            }
            return value;
        } catch {
            throw new Exception ("Could not read value of type 'string'!");
        }
    }

    public Vector3 ReadVector (bool moveReadPosition = true) {
        try {
            return new Vector3 (ReadFloat (), ReadFloat (), ReadFloat ());
        } catch {
            throw new Exception ("Could not read value of type 'Vector3'!");
        }
    }

    public Quaternion ReadQuaternion (bool moveReadPosition = true) {
        try {
            return new Quaternion (ReadFloat (), ReadFloat (), ReadFloat (), ReadFloat ());
        } catch {
            throw new Exception ("Could not read value of type 'Quaternion'!");
        }
    }

    public Item ReadItem (bool moveReadPosition = true) {
        try {
            return ItemDatabase.GetItem (ReadInt (), ReadInt ());
        } catch {
            throw new Exception ("Could not read value of type 'Item'!");
        }
    }

    public Inventory ReadInventory (bool moveReadPosition = true) {
        try {
            var inventory = new Inventory ();
            int itemCount = ReadInt ();
            for (int i = 0; i < itemCount; i++) {
                inventory.AddItem (ReadItem ());
            }
            return inventory;
        } catch {
            throw new Exception ("Could not read value of type 'Inventory'!");
        }
    }
    #endregion
}