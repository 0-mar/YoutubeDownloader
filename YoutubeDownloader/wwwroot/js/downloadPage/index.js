import Utils from "../utils.js";

async function downloadAudio(){
    const url = document.getElementById('video-url').getAttribute('href');
    
    try {
        await Utils.downloadData("/Download/GetResource", {
            url: url
        });
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        document.getElementById("toast--error").hidden = false;
    }
    
}

downloadAudio();




/*const url = "/Download/GetResource?" + new URLSearchParams({
    url: "@Html.Raw(Model.Url)"
}).toString();

const headers = {
    "Content-type": "application/json"
};

fetch(url, {
    method: "GET",
    headers: headers
})
    .then(response => {
        if (!response.ok) {
            document.getElementById("toast--error").hidden = false;
            return;
        }

        response.blob().then((blob) => {
            const downloadUrl = URL.createObjectURL(blob);
            const contentDisposition = response.headers.get("content-disposition");
            let fileName = "audio.mp3";
            const filenameRegex = /filename="([^"]+)"/;
            const match = filenameRegex.exec(contentDisposition);
            if (match && match[1]) {
                fileName = match[1];
            }
            console.log(fileName);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = downloadUrl;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            URL.revokeObjectURL(downloadUrl);
        });
    })
    .catch(error => {
        console.error('There was a problem with the fetch operation:', error);
        document.getElementById("toast--error").hidden = false;
    });*/