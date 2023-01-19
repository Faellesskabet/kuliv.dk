window.getDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};


function ShowMaps() {
    const maps = new okapi.Initialize({});
}

function removeBackgroundImage() {
    document.body.style.background = 'none';
}

setTitle = (title) => {
    document.title = title;
};

setDescription = (description) => {
    document.getElementById("meta-description").setAttribute("content", description)
};