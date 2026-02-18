using Newtonsoft.Json.Linq;
using System.Net;

namespace USGS_Earthquake_Catalog
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        // Button click event to fetch earthquake data
        private async void BtnFind_Clicked(object sender, EventArgs e)
        {
            // Check if all input fields are filled
            if (!string.IsNullOrWhiteSpace(EnterStartDate.Text))
            {
                if (!string.IsNullOrWhiteSpace(EnterEndDate.Text))
                {
                    if (!string.IsNullOrWhiteSpace(EnterEarthquakeSize.Text))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            try
                            {
                                // Call the API using user input values
                                string json = wc.DownloadString($"https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime={EnterStartDate.Text}&endtime={EnterEndDate.Text}&minmagnitude={EnterEarthquakeSize.Text}");

                                // Parse the JSON data
                                JObject jo = JObject.Parse(json);
                                JObject meta = JObject.Parse(jo["metadata"].ToString());

                                // Get number of earthquakes from metadata
                                int count = 0;
                                int.TryParse(meta["count"].ToString(), out count);

                                // Get features array from JSON
                                JArray features = JArray.Parse(jo["features"].ToString());

                                // Store each feature into a new JArray
                                JArray eQjArray = new JArray();
                                for (int i = 0; i < count; i++)
                                {
                                    eQjArray.Add(features[i]);
                                }

                                // Store values in a list of Earthquake objects
                                List<Earthquake> eqList = new List<Earthquake>();
                                int idx = 1;
                                foreach (var eq in eQjArray)
                                {
                                    JObject eqProps = JObject.Parse(eq["properties"].ToString());

                                    Earthquake earthquake = new Earthquake()
                                    {
                                        EQid = idx,
                                        EQSize = double.Parse(eqProps["mag"].ToString()),
                                        EQLocation = eqProps["place"].ToString()
                                    };
                                    eqList.Add(earthquake);
                                    idx++;
                                }

                                // Choose one random earthquake from the list
                                if (eqList.Count > 0)
                                {
                                    Random random = new Random();
                                    int randIndex = random.Next(0, eqList.Count);
                                    Earthquake displayEQ = eqList[randIndex];

                                    lblResults.Text = $"There were {count} earthquakes during this time.\n\n" +
                                                      $"Details of one of them:\nPlace: {displayEQ.EQLocation}, \nMagnitude: {displayEQ.EQSize}.";
                                }
                                else
                                {
                                    lblResults.Text = "No earthquakes found.";
                                }
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Error", ex.Message, "Close");
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Please enter earthquake size", "Close");
                    }
                }
                else
                {
                    await DisplayAlert("Invalid Input", "Please enter end date", "Close");
                }
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter start date", "Close");
            }
        }
    }
}
