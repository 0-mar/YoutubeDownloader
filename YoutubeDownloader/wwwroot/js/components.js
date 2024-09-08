import Utils from "./utils.js";

const Components = (function () {
    const videoCard = function (videoData) {
        const container = document.createElement('div');
        container.className = "video-card"

        const title = document.createElement('h3');
        title.textContent = videoData.title;
        const thumbnail = document.createElement("img");
        thumbnail.src = videoData.thumbnailUrl;
        const author = document.createElement('h4');
        author.textContent = videoData.author;
        const url = document.createElement('a');
        url.href = videoData.url;
        url.textContent = videoData.url;
        const duration = document.createElement('p');
        duration.textContent = videoData.duration;

        container.appendChild(title);
        container.appendChild(author);
        container.appendChild(thumbnail);
        container.appendChild(url);
        container.appendChild(duration);

        return container;
    }
    
    const playlistCard = function (videoData) {
        const container = document.createElement('div');
        container.className = "playlist-card"

        const vCard = videoCard(videoData);
        const checkBox = document.createElement('input')
        checkBox.type = 'checkbox';
        checkBox.videoUrl = videoData.url;
        checkBox.id = videoData.url;
        checkBox.value = videoData.title;
        
        container.appendChild(checkBox);
        container.appendChild(vCard);

        return container;
    }
    
    const downloadVideoCard = function (videoData) {
        const card = videoCard(videoData);

        const downloadAudioButton = document.createElement('button');
        downloadAudioButton.className = "video-download-btn"
        downloadAudioButton.textContent = 'Download audio';
        downloadAudioButton.addEventListener('click', () => Utils.redirect(`Download?url=${videoData.url}&type=audio`));

        const downloadVideoButton = document.createElement('button');
        downloadVideoButton.className = "video-download-btn"
        downloadVideoButton.textContent = 'Download video with audio';
        downloadVideoButton.addEventListener('click', () => Utils.redirect(`Download?url=${videoData.url}&type=video`));

        card.appendChild(downloadAudioButton);
        card.appendChild(downloadVideoButton);
        
        return card;
    }
    
    const historyRecordCard = function (historyRecordData) {
        const container = document.createElement('div');
        container.className = "video-card";

        const date = document.createElement('p');
        date.textContent = Utils.convertDateFromIso(historyRecordData.downloadedOn);

        const title = document.createElement('h3');
        title.textContent = historyRecordData.videoTitle;

        const url = document.createElement('a');
        url.href = historyRecordData.videoUrl;
        url.textContent = historyRecordData.videoUrl;

        const thumbnail = document.createElement("img");
        thumbnail.src = historyRecordData.thumbnailUrl;

        const downloadAgainButton = document.createElement('button');
        downloadAgainButton.className = "video-download-btn";
        downloadAgainButton.textContent = 'Download again';
        downloadAgainButton.addEventListener('click', () => Utils.redirect(`Download?url=${videoData.url}&type=audio`));

        container.appendChild(date);
        container.appendChild(title);
        container.appendChild(url);
        container.appendChild(thumbnail);
        container.appendChild(downloadAgainButton);
        
        return container
    }
    
    const loadingSkeleton = function () {
        const container = document.createElement('div');
        container.className = "loading-skeleton";

        const text = document.createElement('p');
        text.textContent = "Loading...";
        
        container.appendChild(text);
        
        return container;
    }

    const noMoreDataCard = function () {
        const container = document.createElement('div');
        container.className = "no-more-data-card";

        const text = document.createElement('p');
        text.textContent = "No more data to load";

        container.appendChild(text);
        
        return container;
    }
    
    return {
        downloadVideoCard,
        playlistCard,
        historyRecordCard,
        loadingSkeleton,
        noMoreDataCard
    }
})();

export default Components;

