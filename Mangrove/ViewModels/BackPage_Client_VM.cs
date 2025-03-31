namespace Mangrove.ViewModels {
	public class BackPage_Client_VM {
		public string KeyURL { get; set; }
		public long? View { get; set; }

		public BackPage_Client_VM(string KeyURL, long? View = null ) {
			this.KeyURL = KeyURL;
			this.View = View;
		}
	}
}
