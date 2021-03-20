'''
--------------------------------------------------------
Sonic Pi graphical user interface using Unity
--------------------------------------------------------
by Gonzalo María Cidoncha Pérez and Mario Tabasco Vargas
Version 0.0
'''

require 'ostruct'

'''
============================
  Attributes Structures
============================
'''
''' Attribute structure of the SYNTH play action '''
SynthAttributes = Struct.new :action, :synth_name, :notes, :amp, :pan,
:attack, :decay, :sustain, :release, :attack_level, :decay_level, :sustain_level do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'synth'
    self.synth_name ||= 'beep'
    self.notes ||= [52]
    self.amp ||= 1
    self.pan ||= 0
    self.attack ||= 0
    self.sustain ||= 0
    self.release ||= 1
    self.decay ||= 0
    self.attack_level ||= 1
    self.sustain_level ||= 1
    self.decay_level ||= self.sustain_level
  end
end

''' Attribute structure of the SAMPLE play action '''
SampleAttributes = Struct.new :action, :sample_name, :amp, :pan,
  :attack, :decay, :sustain, :release, :attack_level, :decay_level, :sustain_level,
  :lpf, :lpf_attack, :lpf_decay, :lpf_sustain, :lpf_release, :lpf_min, :lpf_init_level, :lpf_release_level, :lpf_sustain_level, :lpf_decay_level, :lpf_attack_level, :lpf_env_curve,
  :hpf, :hpf_max, :hpf_attack, :hpf_decay, :hpf_sustain, :hpf_release, :hpf_init_level, :hpf_release_level, :hpf_sustain_level, :hpf_decay_level, :hpf_attack_level, :hpf_env_curve,
  :rate, :start, :finish, :norm, :pitch, :window_size, :pitch_dis, :time_dis, :compress, :threshold, :clamp_time,
:slope_above, :slope_below, :relax_time do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'sample'
    self.sample_name ||= 'bd_pure'
    self.amp ||= 1
    self.pan ||= 0
    self.attack ||= 0
    self.sustain ||= -1
    self.release ||= 1
    self.decay ||= 0
    self.attack_level ||= 1
    self.sustain_level ||= 1
    self.decay_level ||= self.sustain_level
    # Sample configuration
    # Low Pass Filter
    self.lpf ||= -1
    self.lpf_attack ||= self.attack
    self.lpf_decay ||= self.decay
    self.lpf_sustain ||= self.sustain
    self.lpf_release ||= self.release
    self.lpf_min||= 130
    self.lpf_init_level ||= self.lpf_min
    self.lpf_release_level ||= self.lpf
    self.lpf_sustain_level ||= self.lpf_release_level
    self.lpf_decay_level ||= self.lpf_sustain_level
    self.lpf_attack_level ||= self.lpf_decay_level
    self.lpf_env_curve ||= 2
    # High Pass Filter
    self.hpf = -1
    self.hpf_max = 200
    self.hpf_attack ||= self.attack
    self.hpf_decay ||= self.decay
    self.hpf_sustain ||= self.sustain
    self.hpf_release ||= self.release
    self.hpf_init_level ||= 130
    self.hpf_release_level ||= self.hpf
    self.hpf_sustain_level ||= self.hpf_release_level
    self.hpf_decay_level ||= self.hpf_sustain_level
    self.hpf_attack_level ||= self.hpf_decay_level
    self.hpf_env_curve ||= 2
    # Other sample attributes
    self.rate = 1
    self.start = 0
    self.finish = 1
    self.norm = 0
    self.pitch = 0
    self.window_size = 0.2
    self.pitch_dis = 0.0
    self.time_dis = 0.0
    self.compress = 0
    self.threshold = 0.2
    self.clamp_time = 0.01
    self.slope_above = 0.5
    self.slope_below = 1
    self.relax_time = 0.01
  end
end

''' Attribute structure of the SLEEP action '''
SleepAttributes = Struct.new :action, :sleep_duration do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'sleep'
    self.sleep_duration ||= 1
  end
end


'''
============================
  Command Struct
============================
  Parameters:
  - loop_id: The index of the loop to which the command is attached
- com_id: The index of the command inside the loop command list
- com_attr: Structure containing the attributes of the command

'''

Command = Struct.new(:loop_id, :com_id, :com_attr) do
  # Initialize
  def initialize(*)
    super
    self.loop_id ||= -1
    self.com_id ||= -1
    self.loop_id ||= nil
  end
end

'''
============================
  Methods
============================
'''

'''
Waits for messages from Unity and adds the received commands
- id: Index of the listening loop
- commands: The list of commands of the listening loop
'''
def listenUnityCommand(id, commands)
  # Gets OSC message from Unity
  val = sync"/osc*/sonicpi/unity/trigger"
  
  if val[0] == "stop"
    comAttr = SleepAttributes.new("stop", -1)
    com = Command.new(0, 0, comAttr)
    puts "APPLICATION STOPPED"
  else
    # Parse message to Command
    loopId = val[0]
    comId = val[1]
    i = 2
    puts "Received Command: " + val.to_s
    case val[i]
    # ACTION: SLEEP
    when "sleep"
      comAttr = SleepAttributes.new(val[i], val[i+1])
      # ACTION: PLAY SYNTH
    when "synth"
      numOfNotes = val[i+2] # Number of notes of the sequence to play
      notesToPlay = [] # List of notes to play
      # Add the notes from the message to notesToPlay
      bi = i + 3 # Index of the beginning of the note sequence
      ei = i + 3 + numOfNotes - 1 # Index of the end of the note sequence
      
      for ni in bi..ei do notesToPlay.push(val[ni]) end
      
      pre_i = i
      i = ei + 1
      comAttr = SynthAttributes.new()
      comAttr.action = val[pre_i]
      comAttr.synth_name = val[pre_i + 1]
      comAttr.notes = notesToPlay
      comAttr.amp = val[i]
      comAttr.pan = val[i + 1]
      comAttr.attack = val[i + 2]
      comAttr.sustain = val[i + 3]
      comAttr.release = val[i + 4]
      comAttr.decay = val[i + 5]
      comAttr.attack_level = val[i + 6]
      comAttr.sustain_level = val[i + 7]
      comAttr.decay_level = val[i + 8]
    # SAMPLE
    when "sample"
      comAttr = SampleAttributes.new()
      comAttr.action = val[i]
      comAttr.sample_name = val[i + 1]
      comAttr.amp = val[i + 2]
      comAttr.pan = val[i + 3]
      comAttr.attack = val[i + 4]
      comAttr.sustain = val[i + 5]
      comAttr.release = val[i + 6]
      comAttr.decay = val[i + 7]
      comAttr.attack_level = val[i + 8]
      comAttr.sustain_level = val[i + 9]
      comAttr.decay_level = val[i + 10]
    else
      comAttr = nil
      puts "ERROR: Unknown action name."
    end
      
      com = Command.new(loopId, comId, comAttr)
    end
    
    # Add the command to command list
    commands.push(com)
  end
  
  '''
Process the command list
- id: Index of the processing loop
- commands: The list of commands of the processing loop
'''
  def processCommands(id, commands)
    # Processes each command from the command list in order
    commands.each do |com|
      puts "Processing: " + com.com_attr.action.to_s
      
      case com.com_attr.action
      # ACTION: SLEEP
      when "sleep"
	sleep com.com_attr.sleep_duration
	# ACTION: PLAY SYNTH
      when "synth"
	use_synth com.com_attr.synth_name
        tickNote = com.com_attr.notes.tick
        play tickNote, amp: com.com_attr.amp, pan: com.com_attr.pan, 
            attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release, 
            decay: com.com_attr.decay, attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level, 
            decay_level: com.com_attr.decay_level
	# ACTION: PLAY SAMPLE
      when "sample"
	sample com.com_attr.sample_name, amp: com.com_attr.amp, pan: com.com_attr.pan, attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release, decay: com.com_attr.decay,
            attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level, decay_level: com.com_attr.decay_level
	''' TODO: RESTO DE ATRIBUTOS '''
	# ACTION: WITH FX
      when "fx"
	'''TODO: reproducir secuencia de comandos dentro del bloque with_fx'''
	# STOP
      when "stop"
	stop
      else
	puts "ERROR: Unknown command name. Can't process command."
      end
    end
    sleep 1 # Needs to sleep at least 0.01
  end
  
  
  '''
============================
Listen/Play loops definition
============================
'''
  
  def doListenerLoop(id,commands)
    puts "Listener " + id.to_s
    # LISTENER LOOP
    in_thread do
      loop do
	#puts "id = " + id.to_s
	# Listen commands for command list 'id'
	listenUnityCommand(id, commands[id])
	#puts "Listening (" + id.to_s + ")..."
	sleep 0.1
      end
    end
  end
  
  def doPlayerLoop(id,commands)
    puts "Player " + id.to_s
    # Set the live_loop name
    loopName = "playerLoop#{id.to_s}"
    # PLAYER LOOP
    live_loop loopName do
      #puts "Playing (" + id.to_s + ")..."
      # Process command list number 'id'
      processCommands(id, commands[id])
    end
  end
  
  
  '''
========================================================
Variables and Initialization of the loop rack
========================================================
'''
  use_osc "localhost", 4560
  
  # Number of loops listening to Unity commands
  NUM_LOOPS = 1
  # Command list
  commands = []
  
  # Command list initialization
  for i in 0..(NUM_LOOPS - 1)
    puts i
    commands[i] = []
    doListenerLoop(i, commands) # Starts listening loop
    doPlayerLoop(i, commands)   # Starts playing loop
  end
  
  
  
  
