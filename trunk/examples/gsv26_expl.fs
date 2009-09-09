\ ==============================================================================
\
\       gsv26_expl - the gtk-server notebook example in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/gsv.fs

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  0 value notebook                \ Widgets/events
  0 value window
  0 value exit-button
  0 value next-button
  0 value prev-button
  0 value rotate-button
  0 value border-button
  0 value remove-button
  
  tos-create frame-title          \ Formatter for frame title
  tos-create label-title          \ Formatter for label title
  

  \ Callbacks
  : rotate_book    ( -- = This function rotates the position of the tabs )
    notebook gtk_notebook_get_tab_pos 
    1+ 4 mod 
    notebook gtk_notebook_set_tab_pos
  ;


  : tabsborder_book  ( -- = Add/Remove the page tabs and the borders )
    notebook gtk_notebook_get_show_tabs
    0= notebook gtk_notebook_set_show_tabs
    
    notebook gtk_notebook_get_show_border
    0= notebook gtk_notebook_set_show_border
  ;


  : remove_book    ( -- = Remove a book from the notebook )
    notebook gtk_notebook_get_current_page
    
    notebook gtk_notebook_remove_page
    
    \ Need to refresh the widget -- This forces the widget to redraw itself.
    notebook gtk_widget_queue_draw
  ;


  : next_page      ( -- = Move to next notebook page )
    notebook gtk_notebook_next_page
  ;


  : prev_page      ( -- = Move to previous notebook page )
    notebook gtk_notebook_prev_page
  ;
  
  
  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Notebook" window gtk_window_set_title
    
    10 window gtk_container_set_border_width

    false 6 3 gtk_table_new >r
    r@ window gtk_container_add

    \ Create a new notebook, place the position of the tabs 
    gtk_notebook_new to notebook
    GTK_POS_TOP notebook gtk_notebook_set_tab_pos
    1 0 6 0 notebook r@ gtk_table_attach_defaults
    notebook gtk_widget_show
    
    \ Let's append a bunch of pages to the notebook
    5 0 DO
      frame-title tos-rewrite
      s" Append Frame " frame-title tos-write-string
      I 1+              frame-title tos-write-number

      label-title tos-rewrite
      s" Page " label-title tos-write-string
      I 1+      label-title tos-write-number
 
      frame-title str-get gtk_frame_new >r
      10 r@ gtk_container_set_border_width
      75 100 r@ gtk_widget_set_size_request

      frame-title str-get gtk_label_new >r
      r@ r'@ gtk_container_add
      r> gtk_widget_show 

      label-title str-get gtk_label_new >r 
      r> r@ notebook gtk_notebook_append_page drop
      r> gtk_widget_show

    LOOP

    \ Now let's add a page to a specific spot
    s" Check me please!" gtk_check_button_new_with_label >r
    75 100 r@ gtk_widget_set_size_request
    r@ gtk_widget_show
   
    s" Add page" gtk_label_new >r
    2 r> r> notebook gtk_notebook_insert_page drop
    
    \ Now finally let's prepend pages to the notebook
    5 0 DO
      frame-title tos-rewrite
      s" Prepend Frame " frame-title tos-write-string
      I 1+               frame-title tos-write-number
      
      label-title tos-rewrite
      s" PPage "         label-title tos-write-string
      I 1+               label-title tos-write-number
      
	
      frame-title str-get gtk_frame_new >r
      10 r@ gtk_container_set_border_width
      75 100 r@ gtk_widget_set_size_request
	
      frame-title str-get gtk_label_new >r
      r@ r'@ gtk_container_add
      r> gtk_widget_show
	
      label-title str-get gtk_label_new >r
      r> r@ notebook gtk_notebook_prepend_page drop
      r> gtk_widget_show
    LOOP
    
    \ Set what page to start at (page 4)
    3 notebook gtk_notebook_set_current_page

    \ Create a bunch of buttons 
    s" close" gtk_button_new_with_label to exit-button
    2 1 1 0 exit-button r@ gtk_table_attach_defaults
    exit-button gtk_widget_show
    
    s" next page" gtk_button_new_with_label to next-button
    2 1 2 1 next-button r@ gtk_table_attach_defaults
    next-button gtk_widget_show
    
    s" prev page" gtk_button_new_with_label to prev-button
    2 1 3 2 prev-button r@ gtk_table_attach_defaults
    prev-button gtk_widget_show
    
    s" tab position" gtk_button_new_with_label to rotate-button
    2 1 4 3 rotate-button r@ gtk_table_attach_defaults
    rotate-button gtk_widget_show
    
    s" tabs/border on/off" gtk_button_new_with_label to border-button
    2  1 5 4 border-button r@ gtk_table_attach_defaults
    border-button gtk_widget_show
    
    s" remove page" gtk_button_new_with_label to remove-button
    2 1 6 5 remove-button r@ gtk_table_attach_defaults
    remove-button gtk_widget_show
    
    r> gtk_widget_show            \ table

    window gtk_widget_show
                         
    BEGIN                         \ main loop
      s" WAIT" gtk_server_callback
      event>widget CASE
        window      OF true ENDOF
        exit-button OF true ENDOF
        next-button   OF next_page       false ENDOF
        prev-button   OF prev_page       false ENDOF
        rotate-button OF rotate_book     false ENDOF
        border-button OF tabsborder_book false ENDOF
        remove-button OF remove_book     false ENDOF
      false swap
      ENDCASE
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
