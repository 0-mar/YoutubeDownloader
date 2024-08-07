import RequestApi from "../requestApi.js";
import Components from "../components.js";
import Utils from "../utils.js";

let selections = {};
function attachCheckboxListeners() {
    const checkboxElems = document.querySelectorAll("input[type='checkbox']");

    for (const checkbox of checkboxElems) {
        checkbox.addEventListener("click", function (e) {
            const songsList = document.getElementById("selected-songs-list");
            const songUrl = e.target.id;
            const songTitle = e.target.value;

            if (e.target.checked) {
                selections[songUrl] = {
                    value: songTitle
                };

                const listItem = document.createElement('li');
                listItem.id = songTitle;
                listItem.textContent = songTitle;
                songsList.appendChild(listItem);
            }
            else {
                const listItem = document.getElementById(songTitle);
                songsList.removeChild(listItem);
                delete selections[e.target.id];
            }

            const downloadButton = document.getElementById("playlist-download-btn");
            downloadButton.disabled = Object.keys(selections).length <= 0;
        });
    }
}

function createSelectButton(selectionMode) {
    const btn = document.createElement('button');
    btn.addEventListener('click', function () {
        const checkboxElems = document.querySelectorAll("input[type='checkbox']");
        checkboxElems.forEach(cbx => cbx.checked = selectionMode);
    });
    
    btn.textContent = selectionMode ? "Select all" : "Deselect all";
    
    return btn;
}

function attachUrlInputListener() {
    const urlInput = document.getElementById("playlistUrlInput");
    urlInput.addEventListener("input", async function (evt) {
        const playlistUrlRegex = /^(https:\/\/)?(www\.)?youtube\.com\/playlist\?list=/;
        const url = this.value;

        if (!playlistUrlRegex.test(url)) {
            // TODO visual cue
            console.log("Url is not valid");
            return;
        }

        selections = {};
        const container = document.getElementById("playlist-container");
        container.replaceChildren();
        
        const response = await RequestApi.get("/Playlist/GetPlaylistInfo", {
            url: url
        });

        if (!response.ok) {
            document.getElementById("toast--error").hidden = false;
            return;
        }

        const data = await response.json();
        
        const playlistHeader = document.createElement('div');
        container.appendChild(playlistHeader);
        
        for (const videoData of data.videos) {
            const card = Components.playlistCard(videoData);
            container.appendChild(card);
        }

        attachCheckboxListeners();

        const selectAll = createSelectButton(true);
        const deselectAll = createSelectButton(false);

        const title = document.createElement('h3');
        title.textContent = data.title;

        const thumbnail = document.createElement('img');
        thumbnail.src = data.thumbnailUrl;

        const author = document.createElement('p');
        author.textContent = data.author;

        playlistHeader.appendChild(title);
        playlistHeader.appendChild(author);
        playlistHeader.appendChild(thumbnail);
        playlistHeader.appendChild(selectAll);
        playlistHeader.appendChild(deselectAll);
        
    });
}

attachUrlInputListener();

function attachDownloadButtonListener() {
    const downloadButton = document.getElementById("playlist-download-btn");
    downloadButton.addEventListener('click', async function () {
        const urlList = Object.keys(selections);
        this.disabled = true;
        try {
            await Utils.downloadData("/Playlist/DownloadAudioData", {
                urlList: urlList
            });
        } catch (error) {
            const errorToast = document.getElementById("toast--error");
            
            console.error('There was a problem with downloading playlist:', error);
            errorToast.hidden = false;
            errorToast.textContent = error.message;
        } finally {
            this.disabled = false;
        }
    });
}

attachDownloadButtonListener();