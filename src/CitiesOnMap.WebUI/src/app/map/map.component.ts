import { Component, OnInit } from '@angular/core';
import 'ol/ol.css';
import Map from 'ol/Map';
import View from 'ol/View';
import { OSM, XYZ } from 'ol/source';
import TileLayer from 'ol/layer/Tile';
import { toLonLat } from 'ol/proj';
import { Coordinate } from 'ol/coordinate';

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
          // source: new OSM(),
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
    this.map.on("click", event => {
      let point = this.map.getCoordinateFromPixel(event.pixel);
      console.log(point);
      this.selectedPoint[0] = point[0];
      this.selectedPoint[1] = point[1];
      this.checkAnswer();
    });

  }
  showQuestion: boolean = false;
  showAnswer: boolean = true;
  cityName: string = "Kyiv";
  cityCoordinates: number[] = [30.523333333, 50.450000000];
  selectedPoint: number[] = [0, 0];
  distance: number = 0;
  play() {
    this.showQuestion = true;
  }
  checkAnswer() {
    let l1 = this.cityCoordinates[0];
    let f1 = this.cityCoordinates[1] * Math.PI / 180;
    let l2 = this.selectedPoint[0];
    let f2 = this.selectedPoint[1] * Math.PI / 180;
    let dl = l1 * l2 >= 0
      ? Math.abs(l1 - l2)
      : (Math.abs(l1) + Math.abs(l2)) % 180;
    dl = dl * Math.PI / 180;

    this.distance = Math.atan2(Math.sqrt(Math.pow((Math.cos(f2)*Math.sin(dl)), 2)
      + Math.pow((Math.cos(f1)*Math.sin(f2) - Math.sin(f1)*Math.cos(f2)*Math.cos(dl)), 2)),
      Math.sin(f1)*Math.sin(f2) + Math.cos(f1)*Math.cos(f2)*Math.cos(dl)) * 6371;
    console.log((Math.cos(f2)*Math.sin(dl)));
    console.log((Math.cos(f1)*Math.sin(f2) - Math.sin(f1)*Math.cos(f2)*Math.cos(dl))^2);
    console.log(Math.sin(f1)*Math.sin(f2) + Math.cos(f1)*Math.cos(f2)*Math.cos(dl));
  }
}