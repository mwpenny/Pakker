//IPakInteractor.cs

using System;

namespace Pakker
{
    /// <summary>
    /// A class implementing this interface interacts with .pak files using
    /// a <see cref="PakWorker"/>.
    /// </summary>
    public interface IPakInteractor
    {
        void doneIO(bool success);
        void update();
        PakWorker getWorker();
    }
}
