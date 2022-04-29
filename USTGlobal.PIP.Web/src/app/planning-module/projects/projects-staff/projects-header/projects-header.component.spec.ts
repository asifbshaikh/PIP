import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectsHeaderComponent } from './projects-header.component';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';

describe('ProjectsHeaderComponent', () => {
  let component: ProjectsHeaderComponent;
  let fixture: ComponentFixture<ProjectsHeaderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectsHeaderComponent ],
      imports: [
        DropdownModule,
        FormsModule
      ],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectsHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
