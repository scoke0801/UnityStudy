using System.Collections;
using UnityEngine;

public interface IBikeElement 
{
    void Accept(IVisitor visitor);
}