import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';

@Component({
  selector: 'text-ellipsis',
  templateUrl: './text-ellipsis.component.html',
  styleUrls: ['./text-ellipsis.component.scss']
})
export class TextEllipsisComponent implements OnInit {

  @ViewChild('text') textElement: ElementRef;
  textStr: string;

  ngOnInit() {
    this.textStr = this.textElement.nativeElement.innerText;
  }

}
