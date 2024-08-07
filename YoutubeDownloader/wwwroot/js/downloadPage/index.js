import Utils from "../utils.js";

async function downloadAudio(){
    const url = document.getElementById('video-url').getAttribute('href');
    const type = document.getElementById('type-data').value;
    
    try {
        await Utils.downloadData("/Download/GetResource", {
            url: url,
            type: type
        });
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        document.getElementById("toast--error").hidden = false;
    }
    
}

downloadAudio();