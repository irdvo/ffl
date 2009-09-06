\ ==============================================================================
\
\  gsv10_expl - the gtk-server timer / progress bar example in the ffl
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

  0 value pbar               \ the widget/events
  0 value activity_mode
  0 value timer
  0 value progress
  0 value check1
  0 value check2
  0 value check3
  0 value window
  0 value exit-button


  : widget>event   ( n -- c-addr u = Convert the widget id to an event string )
    s>d tuck dabs <# #s rot sign #>
  ;


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  \ Update the value of the progress bar so that we get some movement
  : progress_timeout  ( -- = process the progress timeout event )
    activity_mode IF
      pbar gtk_progress_bar_pulse
    ELSE
      \ Calculate the value of the progress bar using the value range set in the adjustment object
      
      pbar gtk_progress_bar_get_fraction  0.01E+0 F+ 
      
      fdup 1.0E+0 f> IF
        fdrop 0.0E+0 
      THEN
      
      \ Set the new value
      pbar gtk_progress_bar_set_fraction
    THEN
  ;

  \ Callback that toggles the text display within the progress bar trough
  : toggle_show_text  ( -- = Toggle show text )
    pbar gtk_progress_bar_get_text nip
    IF
      s" "
    ELSE
      s" some text"
    THEN
    pbar gtk_progress_bar_set_text
  ;


  \ Callback that toggles the activity mode of the progress bar 
  : toggle_activity_mode ( -- = Toggle the activity mode )
    activity_mode 0= dup to activity_mode
    
    IF
      pbar gtk_progress_bar_pulse
    ELSE
      0.0E+0 pbar gtk_progress_bar_set_fraction
    THEN
  ;
  
  \ Callback that toggles the orientation of the progress bar 
  : toggle_orientation ( -- = Toggle orientation )
    pbar gtk_progress_bar_get_orientation CASE
      GTK_PROGRESS_LEFT_TO_RIGHT OF GTK_PROGRESS_RIGHT_TO_LEFT pbar gtk_progress_bar_set_orientation ENDOF
      GTK_PROGRESS_RIGHT_TO_LEFT OF GTK_PROGRESS_LEFT_TO_RIGHT pbar gtk_progress_bar_set_orientation ENDOF
    ENDCASE
  ;
  
  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
  
    s" GtkProgressBar" window gtk_window_set_title
 
    true window gtk_window_set_resizable

    0 window gtk_container_set_border_width

    \ Allocate event
    16 g_malloc to progress
    
    \ Add a timer callback to update the value of the progress bar
    progress s" show" window gsv+server-connect 2drop
    s" show" window widget>event s" 100" gtk_server_timeout  event>widget to timer 


    5 false gtk_vbox_new >r
    
    10 r@ gtk_container_set_border_width
    
    r@ window gtk_container_add
    
    \ Create a centering alignment object
    0.0E+0 0.0E+0 0.5E+0 0.5E+0 gtk_alignment_new >r
    5 false false r@ r'@ gtk_box_pack_start
    r@ gtk_widget_show

    \ Create the GtkProgressBar
    gtk_progress_bar_new to pbar
    false to activity_mode
    pbar r> gtk_container_add
    pbar gtk_widget_show

    
    gtk_hseparator_new >r
    0 false false r@ r'@ gtk_box_pack_start
    r> gtk_widget_show

    \ rows, columns, homogeneous
    false 3 2 gtk_table_new >r
    0 true false r@ r'@ gtk_box_pack_start

    \ Add a check button to select displaying of the trough text
    s" Show text" gtk_check_button_new_with_label to check1
    5 5 GTK_EXPAND GTK_FILL or  GTK_EXPAND GTK_FILL or  1 0 1 0 check1 r@ gtk_table_attach
    check1 gtk_widget_show

    \ Add a check button to toggle activity mode
    s" Activity mode" gtk_check_button_new_with_label to check2
    5 5 GTK_EXPAND GTK_FILL or GTK_EXPAND GTK_FILL or 2 1 1 0 check2 r@ gtk_table_attach
    check2 gtk_widget_show


    \ Add a check button to toggle orientation 
    s" Right to Left" gtk_check_button_new_with_label to check3
    5 5 GTK_EXPAND GTK_FILL or GTK_EXPAND GTK_FILL or 3 2 1 0 check3 r@ gtk_table_attach
    check3 gtk_widget_show

    r> gtk_widget_show            \ table

    \ Add a button to exit the program
    s" close" gtk_button_new_with_label to exit-button
    0 false false exit-button r@ gtk_box_pack_start
    \ This makes it so the button is the default.
    \ GTK_WIDGET_SET_FLAGS (button, GTK_CAN_DEFAULT);
    \ This grabs this button to be the default button. Simply hitting
    \ the "Enter" key will cause this button to activate. */
    \ gtk_widget_grab_default (button);
    exit-button gtk_widget_show

    r>          gtk_widget_show   \ vbox

    window      gtk_widget_show
 
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window      OF true ENDOF
        exit-button OF true ENDOF
        progress    OF progress_timeout     false ENDOF
        check1      OF toggle_show_text     false ENDOF
        check2      OF toggle_activity_mode false ENDOF    
        check3      OF toggle_orientation   false ENDOF
        false swap
      ENDCASE
    UNTIL

    timer widget>event gtk_server_timeout_remove type
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
