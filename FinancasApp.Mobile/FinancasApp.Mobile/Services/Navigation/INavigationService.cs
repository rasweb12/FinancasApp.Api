using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancasApp.Mobile.Services.Navigation;

public interface INavigationService
{
    Task NavigateToAsync(string route, bool animate = true);
    Task GoBackAsync(bool animate = true);
}

