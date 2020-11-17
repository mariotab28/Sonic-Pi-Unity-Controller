# TFG
use_osc "localhost", 4560

actions = []

live_loop :listener do
  # Almacena en val lo que recibe por OSC desde Unity
  val = sync"/osc*/sonicpi/unity/trigger"
  puts "Received instruction: " + val[0]
  # Lo añade al array de acciones
  actions.push(val)
  puts actions
end

live_loop :player1 do
  # Reproduce las acciones del array actions en orden
  actions.each do |action|
    puts action[0]
    case action[0]
    when "sleep"
      sleep 1
    when "play"
      play 89, amp: 0.1
    when "stop"
      stop
    else
      puts "ERROR"
    end
  end
  sleep 0.5
end



