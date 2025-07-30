using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Core
{
    /// <summary>
    /// An interface which dictates this node can be observed upon, by providing the blueprint
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMyObservable<T>
    {
        public void Register(T item);
        public void Unregister(T item);
    }
}
