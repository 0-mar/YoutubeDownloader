import Components from "../components.js"
import RequestApi from "../requestApi.js";

document.getElementById("searchButton").addEventListener("click", async function (event) {
    event.preventDefault();
    const queryString = document.getElementById("queryInput").value;
    const loadMoreButton = document.getElementById("loadMoreButton");
    loadMoreButton.hidden = false;
    loadMoreButton.queryString = queryString;
    loadMoreButton.nextPageToken = "";

    const response = await RequestApi.get("/Search/GetQueryResults", {
        searchQuery: queryString
    });
    if (!response.ok) {
        document.getElementById("toast--error").hidden = false;
        return;
    }
    
    const data = await response.json();
    loadMoreButton.nextPageToken = data.nextPageToken;

    const resultContainer = document.getElementById("results");
    resultContainer.replaceChildren();
    for (const videoData of data.videos) {
        const card = Components.downloadVideoCard(videoData);
        resultContainer.appendChild(card);
    }
});

document.getElementById('loadMoreButton').addEventListener('click', async function() {
    const response = await RequestApi.get("/Search/GetQueryResults", {
        searchQuery: this.queryString,
        nextPageToken: this.nextPageToken
    });
    if (!response.ok) {
        document.getElementById("toast--error").hidden = false;
        return;
    }

    const data = await response.json();

    const resultContainer = document.getElementById("results");
    this.nextPageToken = data.nextPageToken;

    for (const videoData of data.videos) {
        const card = Components.downloadVideoCard(videoData);
        resultContainer.appendChild(card);
    }
});