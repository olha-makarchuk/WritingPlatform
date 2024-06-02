import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from "../../_services/user.service";
import { PublicationText } from '../../shared/models/publication-text';

@Component({
  selector: 'app-read-publication',
  templateUrl: './read-publication.component.html',
  styleUrls: ['./read-publication.component.css']
})
export class ReadPublicationComponent implements OnInit {
  publicationText: PublicationText | null = null;

  constructor(private route: ActivatedRoute, private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const publicationId = params['publicationId'];
      const currentPage = params['currentPage'];

      if (publicationId) {
        this.userService.getPublicationText(publicationId, currentPage).subscribe(text => {
          this.publicationText = text;
        });
      }
    });
  }

  getCurrentPageText(): string[] {
    if (this.publicationText) {
      const paragraphsPerPage = this.publicationText.text.split('\n').length / this.publicationText.countOfPages;
      const startIndex = (this.publicationText.currentPage - 1) * paragraphsPerPage;
      const endIndex = startIndex + paragraphsPerPage;
      return this.publicationText.text.split('\n').slice(startIndex, endIndex);
    }
    return [];
  }

  nextPage(): void {
    if (this.publicationText && this.publicationText.currentPage < this.publicationText.countOfPages) {
      const nextPage = this.publicationText.currentPage + 1;
      this.router.navigate([], { queryParams: { currentPage: nextPage }, queryParamsHandling: 'merge' });
    }
  }

  previousPage(): void {
    if (this.publicationText && this.publicationText.currentPage > 1) {
      const previousPage = this.publicationText.currentPage - 1;
      this.router.navigate([], { queryParams: { currentPage: previousPage }, queryParamsHandling: 'merge' });
    }
  }
}
