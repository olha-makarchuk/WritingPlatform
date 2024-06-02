import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllPublicationsComponent } from './all-publications.component';

describe('AllPublicationsComponent', () => {
  let component: AllPublicationsComponent;
  let fixture: ComponentFixture<AllPublicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllPublicationsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AllPublicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
