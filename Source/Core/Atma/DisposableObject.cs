using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Core;

namespace Atma
{
    public abstract class DisposableObject : Object, IDisposable
    {
        private bool isDisposed = false;

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                onDispose();
            }
        }

        ~DisposableObject()
        {
            //Dispose();
        }

        protected abstract void onDispose();
    }
}
