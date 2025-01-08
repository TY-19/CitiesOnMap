import {Component, EventEmitter, OnInit, Output,} from '@angular/core';
import 'ol/ol.css';
import Map from 'ol/Map';
import View from 'ol/View';
import { OSM, XYZ } from 'ol/source';
import TileLayer from 'ol/layer/Tile';
import {Coordinate} from "ol/coordinate";
import {Feature} from "ol";
import {Point} from "ol/geom";

@Component({
  selector: 'citom-map',
  standalone: true,
  imports: [],
  templateUrl: './map.component.html',
  styleUrl: './map.component.scss'
})
export class MapComponent implements OnInit {
  @Output() selectedPoint: EventEmitter<Coordinate> = new EventEmitter<Coordinate>();
  public map!: Map
  ngOnInit(): void {
    this.initializeMap();
    this.listenToClick();
  }
  private initializeMap() : void {
    this.map = new Map({
      layers: [
        new TileLayer({
          source: new XYZ({url: 'https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png'})
        }),
      ],
      target: 'map',
      view: new View({
        projection: 'EPSG:4326',
        center: [0, 0],
        zoom: 2,
        maxZoom: 8,
        extent: [-180, -90, 180, 90]
      })
    });
  }
  private listenToClick() : void {
    this.map.on("click", event => {
      let point = this.map.getCoordinateFromPixel(event.pixel);
      this.selectedPoint.emit(point);
    });
  }
  selectSource(elem: Event): void {
    let sourceType = (elem.target as HTMLSelectElement).value;
    switch (sourceType) {
      case "opentopomap":
        this.map.setLayers([
          new TileLayer({
            source: new XYZ({url: 'https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png'})
          }),
        ])
        break;
      case "OSM":
        this.map.setLayers([
          new TileLayer({
            source: new OSM(),
          }),
        ])
        break;
      default:
        this.map.setLayers([
          new TileLayer({
            source: new OSM(),
          }),
        ])
        break;
    }
  }
}
