namespace DreamLog.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private string description = "";
        private int categoryCount = 0;
        private string version = "";
        private string author = "";
        private int dreamCount = 0;

        public AboutViewModel()
        {
            Title = "About";
        }

        public string Version
        {
            get { return this.version; }
            set
            {
                if (!(this.version?.Equals(value)).GetValueOrDefault())
                    this.SetProperty(ref this.version, value);
            }
        }

        public string Description
        {
            get { return this.description; }
            set
            {
                if (!(this.description?.Equals(value)).GetValueOrDefault())
                    this.SetProperty(ref this.description, value);
            }
        }

        public string Author
        {
            get { return this.author; }
            set
            {
                if (!(this.author?.Equals(value)).GetValueOrDefault())
                    this.SetProperty(ref this.author, value);
            }
        }

        public int CategoryCount
        {
            get { return this.categoryCount; }
            set
            {
                if (this.categoryCount != value)
                    this.SetProperty(ref this.categoryCount, value);
            }
        }

        public int DreamCount
        {
            get { return this.dreamCount; }
            set
            {
                if (this.dreamCount != value)
                    this.SetProperty(ref this.dreamCount, value);
            }
        }
    }
}