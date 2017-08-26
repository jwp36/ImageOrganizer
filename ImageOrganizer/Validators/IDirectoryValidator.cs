using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Validators
{
    public interface IDirectoryValidator
    {
        void Validate(string directoryPath);
    }
}
