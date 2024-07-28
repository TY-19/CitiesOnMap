import { Component, OnInit } from '@angular/core';
import 'ol/ol.css';
import Map from 'ol/Map';
import View from 'ol/View';
import { OSM } from 'ol/source';
import TileLayer from 'ol/layer/Tile';
import { toLonLat } from 'ol/proj';

@Component({
  selector: 'citom-map',
  standalone: true,
  imports: [],
  templateUrl: './map.component.html',
  styleUrl: './map.component.scss'
})
export class MapComponent implements OnInit {
  public map!: Map
  ngOnInit(): void {
    this.map = new Map({
      layers: [
        new TileLayer({
          source: new OSM(),
        }),
      ],
      target: 'map',
      view: new View({ 
        projection: 'EPSG:4326',
        center: [0, 0],
        zoom: 2,
        maxZoom: 2
      })
    });
    this.map.on("click", event => {
      let point = this.map.getCoordinateFromPixel(event.pixel);
      console.log(point);
    });
  }
}