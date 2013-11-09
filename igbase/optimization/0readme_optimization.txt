

This directory contains optimization stuff.

TODO:

	** ANALYSIS class:
		** interface
		** abstract base class
		
		** async interface, will hold analysis reference; usually subclasses will be
		   defined within Analysis class, in order to have full access
		   
		** async abstract base class


	** TEST ANALYSIS class 
		- has some additional functionality 
			- providing test results
			- querying (through properties) if specific results are available
		** interface 
		** abstract base class 
		** some concrete classes
		

	** PARSER IMPLEMENTATION
		- read & write numbers, vectors & matrices
		- read & write optimization results
		

	** ANALYSIS SERVER
		- waits for input
		- reads from input
		- performs analysis
		- writes output
		- notifies output ready
		
		** Implementation for testing: 
			Analysis server having analysis object and serving its functionality
			
		** FILE SYSTEM ANALYSIS SERVER 
			- waits for input ready notification  (waits for creation of file)
			- sets busy flag (writes a file)
			- performs analysis 
			- writes results
			- sets results ready flag
			- unsets ServerBusy flag



	** ANALYSIS CLIENT
		- derived from analysis class?
		- sync/async??
		- what do we do with sync / async operation?
		- one object can be bound to only one server! (in contrast to normal analysis
			function when one object is llowed to serve even analysis results with 
			different parameter dimensions
	
		** FILE SYSTEM ANALYSIS CLIENT
			- contains information about file locations
			Operation: 
				- writes input to file 
				- sets input ready flag (by creating a file)
				- waits for results ready flag (by checking creation of appropriate file)
				- reads analysis results 
				- returns (or executes callbacks in async. ?)

		****** VECTOR FUNCTIONS: *******
		
			TENDENCY: UNIFIED FRAMEWORK FOR VECTOR FUNCTIONS AND LINEAR APPROXIMATION BASES
			
			REQUIREMENTS: Make possible very efficient iterations over values, hessians and gradients
			such as with quadratic base functions
			
					
				POSSIBLE SOLUTION for COMPONENT FUNCTIONS AND ITERATORS:
				Component functions AND iterators will take as parameters
				FUNCTION and its RESULT object. 
				Result object will also take the role of parameters (since parameters are on the 
				result structure).
				Then, it will be on component function to decide whether to read components from results
				or they can be calculated directly (without calculating ALL results). If a component 
				function will rely on results structure, then it will also take care that results are 
				calculated before component function will fetch values.
				The necessary branch can be eliminated by requesting that results ARE evaluated, however
				this is probably not necessary!
		

	
		****** LINEAR APPROXIMATIONS ******
		
		** BASE FUNCTIONS CLASS
			* INTERFACE
			* ABSTRACT BASE CLASS
			
		
		methods: 
			** value of individual
			** collective value
			** gradient of individual
			** gradient component of individual
			** collective gradient
			** hessian of individual
			** hessian component of individual
			** collective hessians 
			
			** can get ref. to scalarfunction for each individual, 
			   or ref. to vectorfunction for each individual
			   
			WARNING:
				- how to handle lots of zeros??
				- define GRADIENT & HESSIAN iterators with callbacks?
					- define as separate objects?? (to avoid conflicts),
						have an instance methods such as GETGRADIENTITERATOR?
						example:
						HessianIterator getHessianIterator();
						Interface IHessianIterator {
							
							// Code below will be executed in every iteration;
							// it will execute a delegate! 
							HessianComponentIteration(BasefunctionCollection basefunc, // to obtain other values
								IVector param,
								int whichFunction, int rowNum, int columnNum, 
								double value)
							}
							
				- problem will be with functions that do not have 
					efficient component getters, for these functions a new analysis function 
					will be created that will work on existent results!!!
						
		
		* PROXY BASE FUNCTIONS BASE CLASS 
