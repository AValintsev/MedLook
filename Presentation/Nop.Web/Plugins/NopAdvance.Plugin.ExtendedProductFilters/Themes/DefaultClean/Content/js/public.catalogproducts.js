var CatalogProducts = {
  settings: {
    ajax: false,
    fetchUrl: false,
    browserPath: false
  },

  params: {
    jqXHR: false,
  },

  init: function (settings) {
    this.settings = $.extend({}, this.settings, settings);
  },

  getProducts: function (pageNumber, lazyLoading) {
    if (this.params.jqXHR && this.params.jqXHR.readyState !== 4) {
      this.params.jqXHR.abort();
    }

    var urlBuilder = createProductsURLBuilder(this.settings.browserPath);


    var beforePayload = {
      urlBuilder
    };
    $(this).trigger({ type: "before", payload: beforePayload });

    if (!lazyLoading && pageNumber) {
      urlBuilder.addParameter('pagenumber', pageNumber);
    }

    this.setBrowserHistory(urlBuilder.build());

    if (lazyLoading && pageNumber) {
      urlBuilder.addParameter('pagenumber', pageNumber);
    }

    if (!this.settings.ajax) {
      setLocation(urlBuilder.build());
    }
    else {
      this.setLoadWaiting(1);

      var self = this;

      this.params.jqXHR = $.ajax({
        cache: false,
        url: urlBuilder.addBasePath(this.settings.fetchUrl).build(),
        type: 'GET',
        success: function (response) {
          if (lazyLoading) {
            /* Remove old pager */
            $('.pager').remove();
            /* Append the result */
            $('.products-wrapper').append(response);

            if ($('.products-wrapper .item-grid').length > 1) {
              /* Extract the products list from response html and append it to old item grid */
              $('.products-wrapper .item-grid:first').append($('.products-wrapper .item-grid:last .item-box'));
              /* Remove the redundant data */
              var prodlist = $('.products-wrapper .product-list:last');
              var prodgrid = $('.products-wrapper .product-grid:last');
              if (prodlist.length > 0) prodlist.remove();
              if (prodgrid.length > 0) prodgrid.remove();
            }
          }
          else {
            $('.products-wrapper').html(response);
          }
          $(self).trigger({ type: "loaded" });
        },
        error: function () {
          $(self).trigger({ type: "error" });
        },
        complete: function () {
          self.setLoadWaiting();
        }
      });
    }
  },

  setLoadWaiting(enable) {
    var $busyEl = $('.ajax-products-busy');
    if (enable) {
      $busyEl.show();
    } else {
      $busyEl.hide();
    }
  },

  setBrowserHistory(url) {
    window.history.replaceState({ path: url }, '', url);
  }
}

function createProductsURLBuilder(basePath) {
  return {
    params: {
      basePath: basePath,
      query: {}
    },

    addBasePath: function (url) {
      this.params.basePath = url;
      return this;
    },

    addParameter: function (name, value) {
      this.params.query[name] = value;
      return this;
    },

    build: function () {
      var query = $.param(this.params.query);
      var url = this.params.basePath;

      return url.indexOf('?') !== -1
        ? url + '&' + query
        : url + '?' + query;
    }
  }
}