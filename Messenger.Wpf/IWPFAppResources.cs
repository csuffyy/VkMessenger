using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmService
{
    public interface IWPFAppResources
    {
        string TooShortPasswordMessage { get; }
        string IncorrectLoginMessage { get; }
    }
}
