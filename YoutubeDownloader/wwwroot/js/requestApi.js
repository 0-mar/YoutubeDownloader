const RequestApi = (function () {
    const get = async function (url, params) {
        const headers = {
            "Content-type": "application/json"
        };
        
        let urlParams = new URLSearchParams();
        for (const [key, value] of Object.entries(params)) {
            if (Array.isArray(value)) {
                value.forEach(x => urlParams.append(key, x));
            } else {
                urlParams.append(key, value);
            }
        }
        
        const newUrl = url + '?' + urlParams.toString();

        return await fetch(newUrl, {
            method: "GET",
            headers: headers
        });
    }

    const post = async function (url, params) {
        const headers = {
            "Content-type": "application/json"
        };

        const body = JSON.stringify(params);
        return await fetch(url, {
            method: "POST",
            headers: headers,
            body: body
        });
    }
    
    return {
      get,
      post  
    };
})();

export default RequestApi;