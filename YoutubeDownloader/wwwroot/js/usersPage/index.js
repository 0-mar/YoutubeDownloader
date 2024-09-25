import RequestApi from "../requestApi.js";
import Components from "../components.js";


let lastId = null
let filters = {
    userName: null,
    email: null,
    startDate: null,
    endDate: null,
    sortField: "username",
    sortOrder: "asc"
}

async function loadUsers () {
    const container = document.getElementById('user-container');
    
    const skeleton = Components.loadingSkeleton();
    container.appendChild(skeleton);

    const response = await RequestApi.get('/Admin/Users/GetUserData', {
        filters: filters,
        lastId: lastId,
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

    data.page.forEach(user => {
        const card = Components.userCard(user);
        container.appendChild(card);
    });

}

const onScrollHandler = async (event) => {
    const {scrollHeight, scrollTop, clientHeight} = event.target;

    if (Math.abs(scrollHeight - clientHeight - scrollTop) < 1) {
        await loadUsers();
    }
}

const container = document.getElementById('user-container');
loadUsers().then(xd => {
    container.addEventListener('scroll', onScrollHandler);
});