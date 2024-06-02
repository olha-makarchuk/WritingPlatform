import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopBooksComponent } from './top-books.component';

describe('TopBooksComponent', () => {
  let component: TopBooksComponent;
  let fixture: ComponentFixture<TopBooksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TopBooksComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TopBooksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
