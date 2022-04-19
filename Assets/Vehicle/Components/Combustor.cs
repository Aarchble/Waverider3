using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combustor : Component
{
    Deflect Isolator;
    Combust Injection;

    public Combustor(NearStream stream, Stream InStream, Fuel fuel)
    {
        Isolator = InStream is NearStream nearIn ? new(nearIn, stream) : new((FreeStream)InStream, stream);
        Injection = new(fuel);
    }


}
