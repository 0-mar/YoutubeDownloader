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
        author.textContent = videoData.duration;

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

        const downloadButton = document.createElement('button');
        downloadButton.className = "video-download-btn"
        downloadButton.textContent = 'Download';
        downloadButton.addEventListener('click', () => Utils.redirect(`Download?url=${videoData.url}`));

        card.appendChild(downloadButton);
        
        return card;
    }
    
    return {
        downloadVideoCard,
        playlistCard
    }
})();

export default Components;

