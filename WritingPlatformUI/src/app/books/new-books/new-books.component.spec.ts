import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewBooksComponent } from './new-books.component';

describe('NewBooksComponent', () => {
  let component: NewBooksComponent;
  let fixture: ComponentFixture<NewBooksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewBooksComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NewBooksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
