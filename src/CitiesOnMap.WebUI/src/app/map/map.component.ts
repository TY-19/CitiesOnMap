import {Component, EventEmitter, OnInit, Output,} from '@angular/core';
import 'ol/ol.css';
import Map from 'ol/Map';
import View from 'ol/View';
import { OSM, XYZ } from 'ol/source';
import TileLayer from 'ol/layer/Tile';
import {Coordinate} from "ol/coordinate";
import VectorSource from "ol/source/Vector";
import VectorLayer from "ol/layer/Vector";
import {Circle, Fill, Stroke, Style} from "ol/style";
import Feature, {FeatureLike} from 'ol/Feature';
import {LineString, Point} from 'ol/geom';

@Component({
  selector: 'citom-map',
  standalone: true,
  imports: [],
  templateUrl: './map.component.html',
  styleUrl: './map.component.scss'
})
export class MapComponent implements OnInit {
  @Output() selectPoint: EventEmitter<Coordinate> = new EventEmitter<Coordinate>();
  private selectedPoint: Coordinate | null = null;
  public map!: Map;
  private vectorSource!: VectorSource;
  private vectorLayer!: VectorLayer<VectorSource>;
  private selectedPointStyle: Style = new Style({
    image: new Circle({
      radius: 6,
      fill: new Fill({color: 'blue'}),
      stroke: new Stroke({
        color: 'white',
        width: 2
      })
    })
  });
  private answerPointStyle: Style = new Style({
    image: new Circle({
      radius: 6,
      fill: new Fill({color: 'green'}),
      stroke: new Stroke({
        color: 'white',
        width: 2
      })
    })
  });
  private lineStyle: Style = new Style({
    stroke: new Stroke({
      color: 'orange',
      width: 4
    })
  });

  ngOnInit(): void {
    this.initializeMap();
    this.listenToClick();
  }
  public displayAnswerPoint(point: Coordinate) {
    const feature = new Feature({
      geometry: new Point(point),
      type: 'answer'
    });

    this.vectorSource.addFeature(feature);

    if(this.selectedPoint) {
      const line = new Feature({
        geometry: new LineString([this.selectedPoint, point]),
        type: 'line'
      });
      this.vectorSource.addFeature(line);
    }
  }
  public clearPoints() {
    this.vectorSource.clear();
  }

  private initializeMap() : void {
    this.vectorSource = new VectorSource();
    this.vectorLayer = new VectorLayer({
      source: this.vectorSource,
      style: feature => this.getStyle(feature.get('type'))
    });

    this.map = new Map({
      layers: [
        new TileLayer({
          source: new XYZ({url: 'https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png'})
        }),
        this.vectorLayer
      ],
      target: 'map',
      view: new View({
        projection: 'EPSG:4326',
        center: [0, 0],
        zoom: 2,
        maxZoom: 32,
        extent: [-200, -90, 200, 90]
      })
    });
  }
  private listenToClick() : void {
    this.map.on("click", event => {
      this.selectedPoint = this.map.getCoordinateFromPixel(event.pixel);
      this.selectPoint.emit(this.selectedPoint);

      this.vectorSource.clear();

      const feature = new Feature({
        geometry: new Point(this.selectedPoint),
        type: 'selected'
      });

      this.vectorSource.addFeature(feature);
    });
  }
  private getStyle(type: string): Style {
    let style: Style;
    switch (type) {
      case 'answer':
        style = this.answerPointStyle;
        break;
      case 'selected':
        style = this.selectedPointStyle;
        break;
      case 'line':
        style = this.lineStyle;
        break;
      default:
        style = this.selectedPointStyle;
        break;
    }
    return style;
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
