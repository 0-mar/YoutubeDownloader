import RequestApi from "./requestApi.js";

const Utils = (function () {
    function redirect(endpoint){
        const paths = window.location;
        window.location.href = `${paths.origin}/${endpoint}`
    }
    
    async function downloadData(url, params) {
        const response = await RequestApi.get(url, params);

        if (!response.ok) {
            throw new Error("Error while downloading data from " + url);
        }

        const blob = await response.blob();

        const downloadUrl = URL.createObjectURL(blob);
        const contentDisposition = response.headers.get("content-disposition");
        let fileName = "unnamed";
        const filenameRegex = /attachment; *filename="?(.+)"?;/;
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
    }
    
    function convertDateFromIso(dateString) {
        const date = new Date(dateString);

        const formattedDate = new Intl.DateTimeFormat('en-GB', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
        }).format(date);

        const formattedTime = new Intl.DateTimeFormat('en-GB', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: false
        }).format(date);

        return `${formattedDate} ${formattedTime}`;
    }
    
    return {
        redirect,
        downloadData,
        convertDateFromIso
    }
})();

export default Utils;