using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Client.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public class OfferDetailsPanel : MonoBehaviour
	{
		private const string DIALOG_TITLE = "Cancel Your Trade Offer?";
		private const string DIALOG_CONTENT = "Would you like to cancel your \"{0}\" {1} Offer?";
		private const string DIALOG_KEEP_1 = "Keep Offer";
		private const string DIALOG_REMOVE_2 = "Remove Offer";

		public bool HasOffer;

		private Transform _placeholder;
		private Transform _offerDetails;

		private Button _cancelOrderButton;
		private Image _itemImage;
		private Image _progressStatus;
		private Text _title;
		private Text _itemName;
		private Text _totalPrice;

		public ITradeItem TradeOffer;
		public TradeInterfaceType Type;

		public void Awake()
		{
			_placeholder = transform.Find("Placeholder");
			_offerDetails = transform.Find("OfferDetails");

			_cancelOrderButton = _offerDetails.Find("CancelOrderButton").GetComponent<Button>();
			_itemImage = _offerDetails.Find("ItemImage").GetComponent<Image>();
			_progressStatus = _offerDetails.Find("ProgressStatus").GetComponent<Image>();
			_title = _offerDetails.Find("Header").GetComponentInChildren<Text>();
			_itemName = _offerDetails.Find("ItemImage").Find("ItemName").GetComponent<Text>();
			_totalPrice = _offerDetails.Find("ItemImage").Find("TotalPrice").GetComponent<Text>();

			_cancelOrderButton.onClick.AddListener(OnCancelClick);
		}

		public void Init(TradeInterfaceType type, ITradeItem trade)
		{
			HasOffer = true;
			TradeOffer = trade;
			Type = type;

			_placeholder.gameObject.SetActive(false);
			_offerDetails.gameObject.SetActive(true);

			_title.text = $"{Type}";
			_itemName.text = TradeOffer.Name;
			_totalPrice.text = StringUtilities.ConvertIntToLongIntString(TradeOffer.Total);
		}

		public void Reintialize()
		{
			HasOffer = false;
			TradeOffer = null;

			_title.text = "";
			_itemName.text = "";
			_totalPrice.text = "0";

			_placeholder.gameObject.SetActive(true);
			_offerDetails.gameObject.SetActive(false);
		}

		public void OnCancelClick()
		{
			HasOffer = false;
			var go = Instantiate(Resources.Load("DialogPopup"), transform.root) as GameObject;
			var dialogPopup = go.GetComponent<DialogPopup>();
			string content = string.Format(DIALOG_CONTENT, TradeOffer.Name, Type);
			dialogPopup.Init(DIALOG_TITLE, content, DIALOG_KEEP_1, () => { }, DIALOG_REMOVE_2, RemoveOffer);
		}

		public void RemoveOffer()
		{
			Reintialize();
		}
	}
}
