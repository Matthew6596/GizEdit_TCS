using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultFileBlock
{
    protected int blockSize, blockId, blockOffset;

    public virtual void readFromFile(int blockSize, int blockId)
    {
        this.blockSize = blockSize; this.blockId = blockId;
        blockOffset = SceneLoader.ReadLocation;
        Debug.Log("Loaded " + TypeConverter.blockIdToString(blockId) + " of length " + blockSize + " at " + blockOffset);
    }

    /**
     * Read a pointer at the current location.
     *
     * @see DefaultFileBlock#readPointer(int)
     */
    public int readPointer()
    {
        return readPointer(SceneLoader.ReadLocation); //return readPointer(ReadLocation);
    }

    /**
     * Read a pointer at the given offset.
     *
     * This applies the relative pointer if possible.
     */
    public int readPointer(int offset)
    {
        SceneLoader.ReadLocation = offset;
        return SceneLoader.reader.ReadInt32(); //PARSE_PNTR = TRUE --> return temp
    }
}
