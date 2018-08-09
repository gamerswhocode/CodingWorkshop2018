using System;

public interface IPoolable
{
    event Action OnDestroyEvent;
}