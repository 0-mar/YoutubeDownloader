import RequestApi from "../requestApi.js";
import Components from "../components.js";


let lastId = undefined
let lastDownloadDate = undefined

async function loadRecords () {
    const container = document.getElementById('history-container');
    
    const skeleton = Components.loadingSkeleton();
    container.appendChild(skeleton);

    const response = await RequestApi.get('History/GetHistoryRecords', {
        lastId: lastId,
        lastDownloadDate: lastDownloadDate
    });

    if (!response.ok) {
        document.getElementById("toast--error").hidden = false;
        return;
    }

    const data = await response.json();

    container.removeChild(skeleton);

    if (data.length === 0) {
        const label = Components.noMoreDataCard();
        container.appendChild(label);
        container.removeEventListener('scroll', onScrollHandler);
        return;
    }

    lastId = data.page[data.length - 1].id;
    lastDownloadDate = data.page[data.length - 1].downloadedOn;

    data.page.forEach(hr => {
        const card = Components.historyRecordCard(hr);
        container.appendChild(card);
    });

}

const onScrollHandler = async (event) => {
    const {scrollHeight, scrollTop, clientHeight} = event.target;

    if (Math.abs(scrollHeight - clientHeight - scrollTop) < 1) {
        await loadRecords();
    }
}

const container = document.getElementById('history-container');
loadRecords().then(xd => {
    container.addEventListener('scroll', onScrollHandler);
});