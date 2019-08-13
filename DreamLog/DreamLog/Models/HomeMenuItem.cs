using System;
using System.Collections.Generic;
using System.Text;

namespace DreamLog.Models
{
    public enum MenuItemType
    {
        DreamLogEntries,
        DreamCategories,
        DreamLogs,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
