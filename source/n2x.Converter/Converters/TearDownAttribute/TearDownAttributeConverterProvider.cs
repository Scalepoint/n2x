﻿using System.Collections.Generic;
using n2x.Converter.Converters.Common;

namespace n2x.Converter.Converters.TearDownAttribute
{
    public class TearDownAttributeConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new DisposableInterfaceImplementer();
            yield return new TearDownMethodMover();
            yield return new MethodRemover<NUnit.Framework.TearDownAttribute>();
        }
    }
}
