using System;

namespace CitizenMP.Server.Resources
{
    public interface ICallRefHandler
    {
        Delegate GetRef(int reference);

        bool HasRef(int reference, uint instance);
    }
}
