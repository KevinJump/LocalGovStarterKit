/*
 * Simple bit of Javascript, to load the google map and put a marker on it
 *
 * looks for a <div id="map_canvas"> element with data attributes 
 * to draw the map.
 */

function LoadGoogleMaps(cb) {
    google.load('maps', '3', {
        callback: cb,
        other_params: "sensor=false"
    });
}

function AddVenueToMap() {
    if ($("#map_canvas").length) {
        var mapElement = $('#map_canvas');

        var mapZoom = mapElement.data('zoom');
        var mapLat = parseFloat(mapElement.data('lat'));
        var mapLng = parseFloat(mapElement.data('lng'));
        var name = mapElement.data('venue-name');
        var address = mapElement.data('venue-address');

        var infoHtml = name;
        if (address && address.length != 0) {
            infoHtml = name + '<br/>' +
                '<a href="https://maps.google.com/maps/dir//' +
                mapLat + ',' + mapLng + '">directions</a>';
        }

        var latlng = new google.maps.LatLng(mapLat, mapLng);

        var mapOptions = {
            zoom: mapZoom,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        var map = new google.maps.Map(
            document.getElementById("map_canvas"),
            mapOptions);

        // now add the marker....
        var venueMarker = new google.maps.Marker({
            position: latlng,
            map: map,
            title: name
        });

        var infoWindow = new google.maps.InfoWindow({
            content: infoHtml
        });

        google.maps.event.addListener(venueMarker, 'click', function () {
            infoWindow.open(map, venueMarker);
        });
    }
}

$(document).ready(function () {
    LoadGoogleMaps('AddVenueToMap');
});