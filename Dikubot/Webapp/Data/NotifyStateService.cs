using System;

namespace Data;

public class NotifyStateService
{
    public event EventHandler? UserChange;

    public void NotifyUserChange(object sender)
    {
        if (this.UserChange == null)
        {
            return;
        }
        this.UserChange(sender, EventArgs.Empty);
    }
}