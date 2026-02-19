const form = document.getElementById("searchForm");
const result = document.getElementById("result");
const findBtn = document.getElementById("findBtn");

form.addEventListener("submit", async (event) => {
  event.preventDefault();

  const startDate = document.getElementById("startDate").value;
  const endDate = document.getElementById("endDate").value;
  const minMagnitude = document.getElementById("minMagnitude").value;

  if (!startDate || !endDate || !minMagnitude) {
    result.textContent = "Please fill in all fields.";
    return;
  }

  if (new Date(startDate) > new Date(endDate)) {
    result.textContent = "Start date must be before end date.";
    return;
  }

  const url = `https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=${encodeURIComponent(startDate)}&endtime=${encodeURIComponent(endDate)}&minmagnitude=${encodeURIComponent(minMagnitude)}`;

  findBtn.disabled = true;
  findBtn.textContent = "Searching...";
  result.textContent = "Loading...";

  try {
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error(`USGS request failed (${response.status})`);
    }

    const data = await response.json();
    const features = Array.isArray(data.features) ? data.features : [];
    const count = Number.isFinite(data?.metadata?.count) ? data.metadata.count : features.length;

    if (features.length === 0) {
      result.textContent = "No earthquakes found for the selected filters.";
      return;
    }

    const randomIndex = Math.floor(Math.random() * features.length);
    const randomEq = features[randomIndex]?.properties || {};
    const place = randomEq.place || "Unknown location";
    const magnitude = randomEq.mag ?? "Unknown";

    result.textContent =
      `There were ${count} earthquakes during this time.\n\n` +
      `Details of one of them:\n` +
      `Place: ${place}\n` +
      `Magnitude: ${magnitude}`;
  } catch (error) {
    result.textContent = `Error: ${error.message}`;
  } finally {
    findBtn.disabled = false;
    findBtn.textContent = "Find";
  }
});
