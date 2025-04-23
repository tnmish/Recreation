
//Определяем карту, координаты центра и начальный масштаб
var map = L.map('map').setView([56.8516, 60.6122], 15);
var latlng;

//Добавляем на нашу карту слой OpenStreetMap
L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 20,
}).addTo(map);

var drawnItems = new L.FeatureGroup();
var loadedItems = new L.FeatureGroup();
map.addLayer(drawnItems);
map.addLayer(loadedItems);

let drawControlOptions = {
    position: 'topleft',
    draw: {
        polygon: true, // Отключить всё
        polyline: true, 
        rectangle: false, 
        circle: false, 
        marker: true, 
        circlemarker: false 
    },
    edit: {
        featureGroup: drawnItems, // Feature group to store drawn items
        remove: false, // Disable removal of shapes
    }
};

var drawControl = new L.Control.Draw(drawControlOptions);
map.addControl(drawControl);

var addButton = L.Control.extend({
    options: {
        position: 'topright'
    },
    onAdd: function (map) {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control');
        container.innerHTML = `<button type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasScrolling" aria-controls="offcanvasScrolling" onclick="setParkOptions()">Добавить</button>`;
       
        return container;
    },
    onRemove: function (map) { },
});

map.addControl(new addButton());

map.on('draw:created', function (event) {
    const layer = event.layer;             
    drawnItems.addLayer(layer);

    if (layer instanceof L.Polygon) {
        var polygonArea = L.GeometryUtil.geodesicArea(layer.getLatLngs()[0]);
        $("#ParkSquare").val(polygonArea);

    } else if(layer instanceof L.Polyline) {
        var coords = layer.getLatLngs();
        var length = 0;
        for (var i = 0; i < coords.length - 1; i++) {
            length += coords[i].distanceTo(coords[i + 1]);
        }        
        $("#BikeLaneLength").val(length);
    }
});

// Получаем список
async function getList() {
    drawnItems.clearLayers();
    loadedItems.clearLayers();

    const response = await fetch('/Home/GetList');
    const items = await response.json();

    items.forEach(item => {
        if (item.Type == 0) {
            const geometry = JSON.parse(item.Geometry);
            if (geometry.type === 'Polygon') {
                L.polygon(geometry.coordinates, { color: 'green' })
                    .addTo(loadedItems)
                    .bindPopup(`<strong>${item.Name}</strong>
                            <br>Площадь: ${item.Square}
                            <br>Тип: ${item.ItemType.Name}`);
            }
        } else if (item.Type == 1) {
            const geometry_1 = JSON.parse(item.Geometry);
            if (geometry_1.type === 'LineString') {
                L.polyline(geometry_1.coordinates, { color: 'red' })
                    .addTo(loadedItems)
                    .bindPopup(`<strong>${item.Name}</strong>
                            <br>Длина: ${item.Length}
                            <br>Тип: ${item.ItemType.Name}`);
            }
        } else if (item.Type == 2) {
            const geometry_2 = JSON.parse(item.Geometry);
            if (geometry_2.type === 'Point') {
                L.marker(geometry_2.coordinates, { color: 'blue' })
                    .addTo(loadedItems)
                    .bindPopup(`<strong>${item.Name}</strong>
                            <br>Директор: ${item.Director}
                            <br>Цена за час: ${item.PricePerHour}`);
            } else if (geometry_2.type === 'Polygon') {
                L.polygon(geometry_2.coordinates, { color: 'blue' })
                    .addTo(loadedItems)
                    .bindPopup(`<strong>${item.Name}</strong>
                            <br>Директор: ${item.Director}
                            <br>Цена за час: ${item.PricePerHour}`);
            }
        }
    });
};

getList();

function getGeometry() {
    const geometry = drawnItems.toGeoJSON().features[0].geometry;

    //непонятно почему после 100 строчки координаты меняются местамы, костылим их на место
    if (geometry.type == 'Point') {
        var tmp = geometry.coordinates[0];
        geometry.coordinates[0] = geometry.coordinates[1];
        geometry.coordinates[1] = tmp;
    } else if(geometry.type == 'Polygon') {
        let newCoordinates = [];
        geometry.coordinates[0].forEach(coordinate => {
            newCoordinates.push([coordinate[1], coordinate[0]]);
        })
        geometry.coordinates[0] = newCoordinates;
    } else if (geometry.type == 'LineString') {
        let newCoordinates = [];
        geometry.coordinates.forEach(coordinate => {
            newCoordinates.push([coordinate[1], coordinate[0]]);
        })
        geometry.coordinates = newCoordinates;
    }

    return geometry;
}

async function savePark() {

    if (!drawnItems.toGeoJSON().features[0]) {
        alert('Ничего не выбрано на карте');
        return;
    }

    const geometry = getGeometry();
    if (geometry.type !== 'Polygon') {
        alert('Нужно выбрать многоугольник');
        return;
    }
    
    const formData = {
        Name: $('#ParkName').val(),
        ItemTypeId: $('#ParkItemTypeId').val(),
        Geometry: JSON.stringify(geometry),
        Square: $('#ParkSquare').val()        
    };

    $.ajax({
        url: '/Home/AddPark',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.success) {
                alert('Парк успешно добавлен.');                
                getList();
            } else {
                alert(response.message || 'Ошибка при сохранении.');
            }
            getList();
        },
        error: function () {
            alert('Произошла ошибка при отправке запроса.');
        }
    });
}

async function saveBikeLane() {

    if (!drawnItems.toGeoJSON().features[0]) {
        alert('Ничего не выбрано на карте');
        return;
    }

    const geometry = getGeometry();
    if (geometry.type !== 'LineString') {
        alert('Нужно выбрать линию');
        return;
    }

    const formData = {
        Name: $('#BikeLaneName').val(),
        ItemTypeId: $('#BikeLaneItemTypeId').val(),
        Geometry: JSON.stringify(geometry),
        Length: $('#BikeLaneLength').val()
    };

    $.ajax({
        url: '/Home/AddBikeLane',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.success) {
                alert('Велодорожка успешно добавлена.');
                
            } else {
                alert(response.message || 'Ошибка при сохранении.');
            }
            getList();
        },
        error: function () {
            alert('Произошла ошибка при отправке запроса.');
        }
    });
}

async function saveRecreationArea() {

    if (!drawnItems.toGeoJSON().features[0]) {
        alert('Ничего не выбрано на карте');
        return;
    }

    const geometry = getGeometry();
    if (geometry.type !== 'Polygon' && geometry.type !== 'Point') {
        alert('Нужно выбрать маркер или многоугольник');
        return;
    }

    const formData = {
        Name: $('#RecreationAreaName').val(),        
        Geometry: JSON.stringify(geometry),
        Director: $('#Director').val(),
        PricePerHour: $('#PricePerHour').val()
    };

    $.ajax({
        url: '/Home/AddRecreationArea',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.success) {
                alert('Зона отдыха успешно добавлена.'); 

            } else {
                alert(response.message || 'Ошибка при сохранении.');
            }

            getList();
        },
        error: function () {
            alert('Произошла ошибка при отправке запроса.');
        }
    });
}

function setParkOptions() {
    drawControl.options.draw.polygon = true;
    drawControl.options.draw.polyline = false;
    drawControl.options.draw.marker = false;
    map.removeControl(drawControl);

    drawControl = new L.Control.Draw(drawControlOptions);
    map.addControl(drawControl);
    getList();
}

function setBikeLaneOptions() {
    
    drawControlOptions.draw.polygon = false;
    drawControlOptions.draw.polyline = true;
    drawControlOptions.draw.marker = false;
    map.removeControl(drawControl);

    drawControl = new L.Control.Draw(drawControlOptions);
    map.addControl(drawControl);
    getList();
}

function setRecreationAreaOptions() {

    drawControlOptions.draw.polygon = true;
    drawControlOptions.draw.polyline = false;
    drawControlOptions.draw.marker = true;
    map.removeControl(drawControl);

    drawControl = new L.Control.Draw(drawControlOptions);
    map.addControl(drawControl);
    getList();
}