using System;

namespace n2x.Tests
{
    public abstract class Specification : IDisposable
    {
        public Specification()
        {
            Context();
            Because();
        }

        public virtual void Context()
        {
        }

        public virtual void Because()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}