import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatePublicationComponent } from './create-publication.component';

describe('CreatePublicationComponent', () => {
  let component: CreatePublicationComponent;
  let fixture: ComponentFixture<CreatePublicationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreatePublicationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreatePublicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
