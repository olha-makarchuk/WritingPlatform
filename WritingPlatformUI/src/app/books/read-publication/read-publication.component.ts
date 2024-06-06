import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from "../../_services/user.service";
import { Publication } from '../../shared/models/publication';

@Component({
  selector: 'app-read-publication',
  templateUrl: './read-publication.component.html',
  styleUrls: ['./read-publication.component.css']
})
export class ReadPublicationComponent implements OnInit {
  publication: Publication | null = null;
  src: string = ""; 
  fileKey: string | null = null; 
  countOfPages: number = 0; 
  currentPage = 1;
  totalPages: number=0;

  constructor(private route: ActivatedRoute, private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.fileKey = params['fileKey']; 
      if (this.fileKey) { 
        this.src = `https://booksinshops.blob.core.windows.net/books-in-shop/${this.fileKey}`;
      }
      this.countOfPages = params['countOfPages']; 
      if (this.countOfPages) { 
        this.totalPages = this.countOfPages;
      }
    });
  }

  goToPage() {
    if (this.currentPage < 1) {
      this.currentPage = 1;
    } else if (this.currentPage > this.totalPages) {
      this.currentPage = this.totalPages;
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }
}
